﻿using Microsoft.EntityFrameworkCore;
using Requiem.PetProject.Data;
using Requiem.PetProject.Helpers;
using Requiem.PetProject.Interfaces;
using Requiem.PetProject.Models;

namespace Requiem.PetProject.Repository;

public class CommentRepository : ICommentRepository
{
    private readonly ApplicationDbContext _context;
    
    public CommentRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task<List<Comment>> GetAllAsync(CommentQueryObject commentQuery)
    {
        var comments = _context.Comments.Include(a => a.AppUser).AsQueryable();

        if (!string.IsNullOrWhiteSpace(commentQuery.Symbol))
            comments = comments.Where(s => s.Stock.Symbol == commentQuery.Symbol);

        if (commentQuery.IsDescending == true)
            comments = comments.OrderByDescending(c => c.CreatedOn);
        
        return await comments.ToListAsync();
    }

    public async Task<Comment?> GetByIdAsync(Guid id)
    {
        return await _context.Comments.Include(a => a.AppUser).FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<Comment> CreateAsync(Comment commentModel)
    {
        await _context.Comments.AddAsync(commentModel);
        await _context.SaveChangesAsync();
        
        return commentModel;
    }

    public async Task<Comment?> UpdateAsync(Guid id, Comment commentModel)
    {
        var existingComment = await _context.Comments.FindAsync(id);

        if (existingComment == null)
        {
            return null;
        }

        existingComment.Title = commentModel.Title;
        existingComment.Content = commentModel.Content;

        await _context.SaveChangesAsync();

        return existingComment;
    }

    public async Task<Comment?> DeleteAsync(Guid id)
    {
        var commentModel = await _context.Comments.FirstOrDefaultAsync(c => c.Id == id);

        if (commentModel == null)
        {
            return null;
        }

        _context.Remove(commentModel);

        await _context.SaveChangesAsync();

        return commentModel;
    }
}